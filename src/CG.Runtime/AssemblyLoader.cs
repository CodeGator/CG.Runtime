using CG.IO;
using CG.Validations;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace CG.Runtime
{
    /// <summary>
    /// This class contains logic to load, and unload .NET assemblies.
    /// </summary>
    public class AssemblyLoader : AssemblyLoadContext
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        private string _baseDirectory;
        private AssemblyDependencyResolver _resolver;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="AssemblyLoader"/>
        /// class.
        /// </summary>
        /// <param name="baseDirectory">The base directory to use for resolving
        /// assemblies at runtime.</param>
        /// <param name="assemblyContextName">The name of the assembly loading
        /// context.</param>
        /// <param name="isCollectible">True to enable unloading assemblies from
        /// the loading context.</param>
        public AssemblyLoader(
            string baseDirectory,
            string assemblyContextName,
            bool isCollectible
            ) : base(assemblyContextName, isCollectible)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNullOrEmpty(
                baseDirectory,
                nameof(baseDirectory)
                ).ThrowIfInvalidFolderPath(
                    baseDirectory,
                    nameof(baseDirectory)
                    );

            // Save the path.
            _baseDirectory = baseDirectory;

            // Create the dependency resolver.
            _resolver = new AssemblyDependencyResolver(
                baseDirectory
                );
        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="AssemblyLoader"/>
        /// class.
        /// </summary>
        /// <param name="assemblyContextName">The name of the assembly loading
        /// context.</param>
        /// <param name="isCollectible">True to enable unloading assemblies from
        /// the loading context.</param>
        public AssemblyLoader(
            string assemblyContextName,
            bool isCollectible
            ) : base(assemblyContextName, isCollectible)
        {
            // Save the path.
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Create the dependency resolver.
            _resolver = new AssemblyDependencyResolver(
                _baseDirectory
                );
        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="AssemblyLoader"/>
        /// class.
        /// </summary>
        /// <param name="isCollectible">True to enable unloading assemblies from
        /// the loading context.</param>
        public AssemblyLoader(
            bool isCollectible
            ) : base(isCollectible)
        {
            // Save the path.
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Create the dependency resolver.
            _resolver = new AssemblyDependencyResolver(
                _baseDirectory
                );
        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="AssemblyLoader"/>
        /// class.
        /// </summary>
        public AssemblyLoader()
        {
            // Save the path.
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Create the dependency resolver.
            _resolver = new AssemblyDependencyResolver(
                _baseDirectory
                );
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method allows an assembly to be loaded based on it's assembly
        /// name.
        /// </summary>
        /// <param name="assemblyName">The assembly name to user for the operation.</param>
        /// <returns>An <see cref="Assembly"/> object.</returns>
        protected override Assembly Load(
            AssemblyName assemblyName
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(
                assemblyName,
                nameof(assemblyName)
                );

            // Resolve the file path from the assembly name.
            var assemblyPath = _resolver.ResolveAssemblyToPath(
                assemblyName
                );

            // Did we succeed?
            if (false == string.IsNullOrEmpty(assemblyPath))
            {
                // Load the assembly.
                return LoadFromAssemblyPath(
                    assemblyPath
                    );
            }

            // If we get here, we failed to resolve the assembly using the fancy
            //   schmancy .NET resolver object. Let's try to load it the old school
            //   way, instead.

            // Try to build a complete path to the file.
            var completePath = Path.Combine(
                _baseDirectory,
                $"{assemblyName.Name}.dll"
                );

            // Does the file exist?
            if (File.Exists(completePath))
            {
                // Try to load the assembly using the path.
                return Assembly.LoadFrom(completePath);
            }

            // We failed to load the assembly.
            return null;
        }

        // *******************************************************************

        /// <summary>
        /// This method allows an unmanaged library to be loaded, by name.
        /// </summary>
        /// <param name="unmanagedDllName">The assembly name to use for the 
        /// operation.</param>
        /// <returns>An <see cref="IntPtr"/> object.</returns>
        protected override IntPtr LoadUnmanagedDll(
            string unmanagedDllName
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNullOrEmpty(
                unmanagedDllName,
                nameof(unmanagedDllName)
                );

            // Resolve the file path from the library name.
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(
                unmanagedDllName
                );

            // Did we succeed?
            if (false == string.IsNullOrEmpty(libraryPath))
            {
                // Load the library.
                return LoadUnmanagedDllFromPath(
                    libraryPath
                    );
            }

            // We failed to load the library.
            return IntPtr.Zero;
        }

        #endregion
    }

}
