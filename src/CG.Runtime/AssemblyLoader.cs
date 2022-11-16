
namespace System.Runtime;

/// <summary>
/// This class contains logic to load and unload .NET assemblies.
/// </summary>
public class AssemblyLoader : AssemblyLoadContext
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    private string _baseDirectory;
    private string _dynamicDirectory;
    private string _relativeSearchDirectory;
    private string _applicationDirectory;

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

        // Save the values.
        _baseDirectory = baseDirectory;
        _dynamicDirectory = AppDomain.CurrentDomain.DynamicDirectory;
        _relativeSearchDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
        _applicationDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

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
        // Save the values.
        _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _dynamicDirectory = AppDomain.CurrentDomain.DynamicDirectory;
        _relativeSearchDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
        _applicationDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

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
        // Save the values.
        _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _dynamicDirectory = AppDomain.CurrentDomain.DynamicDirectory;
        _relativeSearchDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
        _applicationDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

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
        // Save the values.
        _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _dynamicDirectory = AppDomain.CurrentDomain.DynamicDirectory;
        _relativeSearchDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
        _applicationDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

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
        //   way, instead. Let's probe the base directory first, then the application 
        //   directory, then the search directory, then the dynamic directory.

        // Do we have a base directory?
        if (false == string.IsNullOrEmpty(_baseDirectory))
        {
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
        }

        // Do we have an application directory?
        if (false == string.IsNullOrEmpty(_applicationDirectory))
        {
            // Try to build a complete path to the file.
            var completePath = Path.Combine(
                _applicationDirectory,
                $"{assemblyName.Name}.dll"
                );

            // Does the file exist?
            if (File.Exists(completePath))
            {
                // Try to load the assembly using the path.
                return Assembly.LoadFrom(completePath);
            }
        }

        // Do we have a search directory?
        if (false == string.IsNullOrEmpty(_relativeSearchDirectory))
        {
            // Try to build a complete path to the file.
            var completePath = Path.Combine(
                _relativeSearchDirectory,
                $"{assemblyName.Name}.dll"
                );

            // Does the file exist?
            if (File.Exists(completePath))
            {
                // Try to load the assembly using the path.
                return Assembly.LoadFrom(completePath);
            }
        }

        // Do we have a dynamic directory?
        if (false == string.IsNullOrEmpty(_dynamicDirectory))
        {
            // Try to build a complete path to the file.
            var completePath = Path.Combine(
                _dynamicDirectory,
                $"{assemblyName.Name}.dll"
                );

            // Does the file exist?
            if (File.Exists(completePath))
            {
                // Try to load the assembly using the path.
                return Assembly.LoadFrom(completePath);
            }
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
