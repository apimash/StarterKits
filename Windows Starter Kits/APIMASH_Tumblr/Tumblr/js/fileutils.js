(function () {
    
    WinJS.Namespace.define("bwc", {
        FileUtils : WinJS.Class.define(function () {
            this.defaultOption = Windows.Storage.NameCollisionOption.generateUniqueName;
            this.defaultEncoding = Windows.Storage.Streams.UnicodeEncoding.utf8;

    }, {
        createFolder: function (storageFolder,desiredName) {
            return (storageFolder.createFolderAsync(desiredName));
        },
        
        createFile: function (storageFolder, desiredName, options) {
            if (!options) {
                options = this.defaultOption;
            }
            return storageFolder.createFileAsync(desiredName, options);
        },
   
        appendToFile: function (storageFile,contents,encoding) {
            if (!encoding) {
                encoding = this.defaultEncoding;
            }
            return (Windows.Storage.FileIO.appendTextAsync(storageFile, contents, encoding));
        },
 
        getFolder: function (storageFolder,name) {
            return storageFolder.getFolderAsync(name);
        },
   
        getFile: function (storageFolder,name) {
            return storageFolder.getFileAsync(name);
        },
  
        getFiles: function (storageFolder,fileTypeFilter,isRecursive) {
            var search = Windows.Storage.Search;
            if (!fileTypeFilter) {
                fileTypeFilter = ['*'];
            }
            var query = new search.QueryOptions(search.CommonFileQuery.orderByName, fileTypeFilter);
            if (isRecursive) {
                query.folderDepth = search.FolderDepth.deep;
            }
            return storageFolder.getFilesAsync(query);
           
        },
    
        getFolders: function (storageFolder,query) {
            // sm not sure this is requred
            if (query) {
                return storageFolder.getFoldersAsync(query);
            } else {
                return storageFolder.getFoldersAsync();
            }
        },
    
        renameFolder: function (storageFolder, desiredName,option) {
            if (!option) {
                option = this.defaultOption;
                
            }
            
            return (storageFolder.renameAsync(desiredName));
        },

        renameFile: function (storageFile,desiredName,option) {
            return (storageFile.renameAsync(desiredName, option));

        },
 
        getAllFiles: function (path,mask) {
            console.log("Hello " + this.name);
        },
   
        getAllFilesByType: function (storageFolder,types,isRecursive) {
            
        },
  
        readFile: function (storageFile) {
            return Windows.Storage.FileIO.readTextAsync(storageFile);
        },
   
        writeFile: function (storageFile,contents,encoding) {
            if (!encoding) {
                encoding = this.defaultEncoding;
            }
            return Windows.Storage.FileIO.writeTextAsync(storageFile,contents,encoding);
        },
 
        remove: function (storageFolder,option) {
            return (storageFolder.deleteAsync(option));
        },

        deleteFile: function (storageFile,option) {
            return (storageFile.deleteAsync(option));
        },
        copyFile: function (storageFile,destinationFolder,desiredName,option) {
            if (!option) {
               
            }
            storageFile.copyAsync(destinationFolder, desiredName);
        },
 
        copyFolder: function (path,name) {
            console.log("Hello " + this.name);
        },

        moveFolder: function (path,name) {
            console.log("Hello " + this.name);
        },
        moveFile: function (storageFile,destinationFolder,desiredNewName,option) {
            return storageFile.moveAsync(destinationFolder, desiredNewName, option);
        }
    })
        
    });

   

})();

