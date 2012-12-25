These are the MonoGame binaries, compiled from source on the 'develop3d' branch.  The reason we don't
include it as a submodule is that the MonoGame repository itself contains submodules, and those
submodules take a long time to download.

In addition, the MonoGame binaries provided by the MonoDevelop addin are not of the correct .NET
framework version, and thus do not work on .NET 4.  These binaries however, are compiled for the
.NET framework 4.
