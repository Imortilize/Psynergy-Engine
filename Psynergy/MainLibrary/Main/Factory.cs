using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Psynergy
{
    public class Factory : Singleton<Factory>
    {
        #region Member Variables
        private SortedList<String, Type> m_Classes = new SortedList<String, Type>();

        #region Property registers
        private SortedList<String, PropertyBag> m_ClassProperties = new SortedList<String, PropertyBag>();

        //private SortedList<String, SortedList<String, Action<Vector3>>> m_ClassRegisteredVector3 = new SortedList<String, SortedList<String, Action<Vector3>>>();
        #endregion
        // Cached reference to the containing assembly
        private Assembly m_Assembly;

        // Assembly storage
        Dictionary<AssemblyName, List<Type>> m_AllTypes = new Dictionary<AssemblyName, List<Type>>();

        // Class register resource
        ClassRegistryResource m_ClassResource = null;
        Type m_CurrentRegisterType = null;              // When a class is being registered the type is stored here
        #endregion

        public Factory()
        {
            m_Assembly = Assembly.GetEntryAssembly();
        }

        public override void Initialise()
        {
            if (m_ClassResource == null)
                m_ClassResource = new ClassRegistryResource("Resources/ClassRegistry.xml");

            base.Initialise();

            // Load assemblies
            LoadAssemblies();
        }

        public override void Load()
        {
            // Load the menu resources
            if (m_ClassResource != null)
                m_ClassResource.Load();

            base.Load();
        }

        #region Assembly management
        private void LoadAssemblies()
        {
            if (m_Assembly != null)
            {
                // Get all assembly names
                AssemblyName[] assemblies = m_Assembly.GetReferencedAssemblies();
                Assembly[] assembliesTest = AppDomain.CurrentDomain.GetAssemblies();

                List<Assembly> assemblyList = new List<Assembly>();

                foreach (AssemblyName name in assemblies)
                {
                    String fullName = name.FullName;

                    if (fullName.Contains("Library"))
                    {
                        Assembly assemblyToAdd = Assembly.Load(name);

                        // Add the types
                        List<Type> newTypes = new List<Type>();
                        newTypes.AddRange(assemblyToAdd.GetTypes());

                        // Add the types
                        m_AllTypes.Add(assemblyToAdd.GetName(), newTypes);
                    }
                }

                for (int i = 0; i < assembliesTest.Length; i++)
                {
                    Assembly assembly = assembliesTest[i];

                    String fullName = assembly.GetName().FullName;

                    if (fullName.Contains("Library"))
                    {
                        for (int j = 0; j < m_AllTypes.Count; j++)
                        {
                            AssemblyName assemblyName = m_AllTypes.ElementAt(j).Key;

                            if (assemblyName.Name == assembly.GetName().Name)
                                break;
                            else if (j == (m_AllTypes.Count - 1))
                            {
                                // Add the types
                                List<Type> newTypes = new List<Type>();
                                newTypes.AddRange(assembly.GetTypes());

                                // Add the types
                                m_AllTypes.Add(assembly.GetName(), newTypes);
                            }
                        }
                    }
                }
   
                // Add the excuting library assembly classes too
                List<Type> excuteTypes = new List<Type>();
                excuteTypes.AddRange(m_Assembly.GetTypes());
                m_AllTypes.Add(m_Assembly.GetName(), excuteTypes);
            }
        }

        public Type FindType(String typeName, out Assembly assemblyToUse)
        {
            // Now check for the type
            Type type = null;
            assemblyToUse = null;

            foreach (KeyValuePair<AssemblyName, List<Type>> types in m_AllTypes)
            {
                foreach (Type typeToCheck in types.Value)
                {
                    if (typeToCheck.Name == typeName)
                    {
                        assemblyToUse = Assembly.Load(types.Key);
                        type = typeToCheck;
                        break;
                    }
                }

                if (type != null)
                    break;
            }

            return type;
        }

        public List<Type> GetTypesUsingInterface(String typeName, bool genericArguement)
        {
            // Find the type
            Assembly assembly = null;
            Type interfaceType = FindType(typeName, out assembly);
            List<Type> typesToRet = new List<Type>();// FindType(typeName, out assemblyToUse);     

            foreach (KeyValuePair<AssemblyName, List<Type>> types in m_AllTypes)
            {
                foreach (Type typeToCheck in types.Value)
                {
                    foreach (var i in typeToCheck.GetInterfaces())
                    {
                        if (genericArguement && i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                        {
                            Type test1 = i.GetGenericArguments()[0];

                            if (test1 != null)
                            {
                                typesToRet.Add(typeToCheck);

                                break;
                            }
                        }
                    }
                }
            }

            return typesToRet;
        }

        public object CreateInstance(Type type)
        {
            object instance = null;

            // If it exists...
            if (type != null)
            {
                foreach (AssemblyName assemblyName in m_AllTypes.Keys)
                {
                    Assembly assembly = Assembly.Load(assemblyName);

                    // Try create an instance of the object from this assembly
                    instance = assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                    // If an instance was created then break.
                    if (instance != null)
                        break;
                }

                // check it instantiated ok.
                if (instance == null)
                    throw new NullReferenceException("Null product instance. Unable to create neccesary product class.");
            }

            return instance;
        }
        #endregion

        public void Register(String typeName)
        {
            Assembly assemblyToUse = null;

            // Find the type
            Type type = FindType(typeName, out assemblyToUse);
            
            if (type != null)
            {
                // Now register accordingly
                Register(assemblyToUse, typeName, type);
            }
        }

        private void Register(Assembly assembly, String typeName, Type type)
        {
            Debug.Assert(assembly != null, "Assembly should not NULL when registering a class!");

            if (assembly != null)
            {
                // Check the name hasn't already been used
                if (!m_Classes.ContainsValue(type))
                {
                    // Create a temporary instance of that class...
                    object instance = assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                    if (instance != null)
                    {
                        m_Classes.Add(typeName, type);

                        // Save the current class type being registered
                        m_CurrentRegisterType = type;

                        // All classes derive from GameObject so cast to this
                        RegisterProperties(typeName, (GameObject)instance);

                        // Null the register type
                        m_CurrentRegisterType = null;
                    }
                }
            }
        }

        public GameObject Create(String className)
        {
            if (className == null)
                throw new NullReferenceException("Invalid class name supplied, must be non-null.");

            Type type = null;

            // Try get the class value
            m_Classes.TryGetValue(className, out type);

            // If it exists...
            if (type != null)
            {
                object instance = null;

                foreach (AssemblyName assemblyName in m_AllTypes.Keys)
                {
                    Assembly assembly = Assembly.Load(assemblyName);

                    // Try create an instance of the object from this assembly
                    instance = assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                    // If an instance was created then break.
                    if (instance != null)
                        break;
                }

                // check it instantiated ok.
                if (instance == null)
                    throw new NullReferenceException("Null product instance. Unable to create neccesary product class.");

                //Debug.WriteLine("[OBJECT CREATION] - instance of type " + type.FullName + " created through the factory!");

                // Determine what object to create
                return CreateObject(className, instance);
            }
            else
            {
                return null;
            }
        }

        #region Create decision function
        private GameObject CreateObject(String className, object instance)
        {
            if (instance != null)
            {
                return (GameObject)instance;
            }
            else
                return null;
        }
        #endregion

        public int GetKey(Type type)
        {
            int key = -1;

            if ( m_Classes.ContainsValue(type) )
            {
                key = m_Classes.IndexOfValue( type );
            }

            return key;
        }

        #region Property related Items
        protected virtual void RegisterProperties(String typeName, GameObject t)
        {
            // Register a new class to the class properties
            m_ClassProperties.Add(typeName, new PropertyBag());

            // Register class related properties
            t.RegisterProperties( this );
        }

        public void RegisterInt( String propertyName, String propertySetGet )
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(int), propertyName, propertySetGet);
        }

        public void RegisterFloat( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(float), propertyName, propertySetGet);
        }

        public void RegisterBool( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(bool), propertyName, propertySetGet);
        }

        public void RegisterString( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(String), propertyName, propertySetGet);
        }

        public void RegisterVector2( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(Vector2), propertyName, propertySetGet);
        }

        public void RegisterVector3( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(Vector3), propertyName, propertySetGet);
        }

        public void RegisterColour( String propertyName, String propertySetGet )
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(Color), propertyName, propertySetGet);
        }

        public void RegisterQuat( String propertyName, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (propertyName != null) && (propertyName != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, typeof(Quaternion), propertyName, propertySetGet);
        }

        public void RegisterClass(Type classType, String className, String propertySetGet)
        {
            Debug.Assert(m_CurrentRegisterType != null, "Current class being registered too should not be null!");

            // make sure everything is set as it should be 
            if ((m_CurrentRegisterType != null) && (className != null) && (className != "") && (propertySetGet != null) && (propertySetGet != ""))
                RegisterProperty(m_CurrentRegisterType, classType, className, propertySetGet);
        }

        private void RegisterProperty( Type classType, Type propertyType, String propertyName, String propertySetGet )
        {
            PropertyBag propertyBag = null;

            m_ClassProperties.TryGetValue(classType.Name, out propertyBag);

            // If not null
            if (propertyBag != null)
            {
                // Add to the property bags properties
                propertyBag.Properties.Add(new PropertySpec(propertySetGet, propertyType, classType.Name + ":Properties", propertyName));
            }
        }
        #endregion

        #region Register Setters
        public bool SetProperty(Type type, String propertyName, String value, GameObject invokeNode)
        {
            bool toRet = false;

            // Split the value into seperate numbers ( used for vectors and so forth )
            String[] splitValue = value.Split(new Char[] { ' ' });

            PropertyBag classProperties = null;

            // Try and find the class properties
            m_ClassProperties.TryGetValue(type.Name, out classProperties);

            Debug.Assert(classProperties != null, "Class property bag not found on a node of type " + type.Name + ".");

            // If it was found
            if (classProperties != null)
            {
                // Try and find the property in this bag
                for ( int i = 0; i < classProperties.Properties.Count; i++ )
                {
                    if ( propertyName == classProperties.Properties[i].Description )
                    {
                        toRet = true;

                        // The property has been found 
                        PropertySpec property = classProperties.Properties[i];
                        PropertySpecEventArgs args = null;

                        Type propertyType = property.PropertyType;

                         // Create a new property spec arguement event according to type
                        if (propertyType == typeof(int))
                            args = new PropertySpecEventArgs(property, System.Convert.ToInt32(value));
                        else if (propertyType == typeof(float))
                            args = new PropertySpecEventArgs(property, System.Convert.ToSingle(value));
                        else if (propertyType == typeof(bool))
                            args = new PropertySpecEventArgs(property, System.Convert.ToBoolean(value));
                        else if (propertyType == typeof(String))
                            args = new PropertySpecEventArgs(property, value);
                        else if (propertyType == typeof(Vector2))
                            args = new PropertySpecEventArgs(property, new Vector2(System.Convert.ToSingle(splitValue[0]), System.Convert.ToSingle(splitValue[1])));
                        else if (propertyType == typeof(Vector3))
                            args = new PropertySpecEventArgs(property, new Vector3(System.Convert.ToSingle(splitValue[0]), System.Convert.ToSingle(splitValue[1]), System.Convert.ToSingle(splitValue[2])));
                        else if (propertyType == typeof(Vector4))
                            args = new PropertySpecEventArgs(property, new Vector4(System.Convert.ToSingle(splitValue[0]), System.Convert.ToSingle(splitValue[1]), System.Convert.ToSingle(splitValue[2]), System.Convert.ToSingle(splitValue[3])));
                        else if (propertyType == typeof(Color))
                            args = new PropertySpecEventArgs(property, new Color(System.Convert.ToSingle(splitValue[0]), System.Convert.ToSingle(splitValue[1]), System.Convert.ToSingle(splitValue[2]), System.Convert.ToSingle(splitValue[3])));
                        else if (propertyType == typeof(Quaternion))
                            args = new PropertySpecEventArgs(property, new Quaternion(System.Convert.ToSingle(splitValue[0]), System.Convert.ToSingle(splitValue[1]), System.Convert.ToSingle(splitValue[2]), System.Convert.ToSingle(splitValue[3])));

                        invokeNode.SetProperty(this, args);

                        // Found it so leave the cycle
                        break;
                    }
                 }
            }

            return toRet;
        }

        public bool SetClass(Type type, String propertyName, object value, GameObject invokeNode)
        {
            bool toRet = false;

            PropertyBag classProperties = null;

            // Try and find the class properties
            m_ClassProperties.TryGetValue(type.Name, out classProperties);

            Debug.Assert(classProperties != null, "Class property bag not found on a node of type " + type.Name + ".");

            // If it was found
            if (m_ClassProperties != null)
            {
                // Try and find the property in this bag
                for (int i = 0; i < classProperties.Properties.Count; i++)
                {
                    if (propertyName == classProperties.Properties[i].Description)
                    {
                        toRet = true;

                        // The property has been found 
                        PropertySpec property = classProperties.Properties[i];
                        PropertySpecEventArgs args = null;

                        Type propertyType = property.PropertyType;

                        GameObject classObject = (GameObject)value;

                        // Call the on class set function
                        classObject.OnClassSet(invokeNode);
                        
                        // Get the property spec
                        args = new PropertySpecEventArgs(property, classObject);

                        // Set the property
                        invokeNode.SetProperty(this, args);

                        // Found it so leave the cycle
                        break;
                    }
                }
            }

            return toRet;
        }
        #endregion
    }
}
