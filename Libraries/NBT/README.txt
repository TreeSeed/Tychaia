Current released version is 0.3.2 (18 October 2012).

Named Binary Tag (NBT) is a structured binary file format used by Minecraft.
LibNbt2012 is a small library, written in C# for .NET 2.0+. It provides
functionality to create, load, traverse, modify, and save NBT files and
streams.

LibNbt2012 is based on Erik Davidson's (aphistic's) original LibNbt library,
rewritten by Matvei Stefarov (fragmer) for improved performance, ease of use,
and reliability.


==== FEATURES =================================================================
- Can load and save uncompressed, GZip-, and ZLib-compressed files/streams.
- Can easy create, traverse, and modify NBT documents.
- Simple indexer-based syntax for accessing compound, list, and nested tags.
- Shortcut properties to access tags' values without unnecessary type casts.
- Compound tags implement ICollection<T> and List tags implement IList<T>,
    for easy traversal and LINQ integration.
- Good performance and low memory overhead.
- Built-in pretty printing of individual tags or whole files.
- Every class and method are fully documented and unit-tested.


==== EXAMPLES =================================================================
- Loading a gzipped file:
    NbtFile myFile = new NbtFile("somefile.nbt.gz");
    NbtTag myCompoundTag = myFile.RootTag;

- Accessing tags (long/strongly-typed style):
    int intVal = myCompoundTag.Get<NbtInt>("intTagsName").Value;
    string listItem = myStringList.Get<NbtString>(0).Value;
    byte nestedVal = myCompTag.Get<NbtCompound>("nestedTag")
                              .Get<NbtByte>("someByteTag")
                              .Value;

- Accessing tags (shortcut style):
    int intVal = myCompoundTag["intTagsName"].IntValue;
    string listItem = myStringList[0].StringValue;
    byte nestedVal = myCompTag["nestedTag"]["someByteTag"].ByteValue;

- Iterating over all tags in a compound/list:
    foreach( NbtTag tag in myCompoundTag.Values ){
        Console.WriteLine( tag.Name + " = " + tag.TagType );
    }
    foreach( string tagName in myCompoundTag.Names ){
        Console.WriteLine( tagName );
    }
    for( int i=0; i<myListTag.Count; i++ ){
        Console.WriteLine( myListTag[i] );
    }
    foreach( NbtInt intListItem in myIntList.ToArray<NbtInt>() ){
        Console.WriteLine( listIntItem.Value );
    }

- Constructing a new document
    NbtCompound serverInfo = new NbtCompound("Server");
    serverInfo.Add( new NbtString("Name", "BestServerEver") );
    serverInfo.Add( new NbtInt("Players", 15) );
    serverInfo.Add( new NbtInt("MaxPlayers", 20) );
    NbtFile serverFile = new NbtFile(serverInfo);
    serverFile.SaveToFile( "server.nbt", NbtCompression.None );

- Pretty-printing file structure
    Console.WriteLine( myFile.RootTag.ToString("\t") );
    Console.WriteLine( myRandomTag.ToString("    ") );


==== LICENSING ================================================================
LibNbt2012 keeps LibNbt's original license (LGPLv3). See ./docs/LICENSE


==== VERSION HISTORY ==========================================================
See ./docs/Changelog
