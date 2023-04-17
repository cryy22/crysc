var IndexedDBSync = {
  Crysc_SyncDB: function() {
    FS.syncfs(false, err => { console.warn(err); }); 
  }
};

mergeInto(LibraryManager.library, IndexedDBSync);
