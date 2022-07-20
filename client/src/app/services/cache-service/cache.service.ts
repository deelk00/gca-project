import { Injectable } from '@angular/core';

export enum StorageType {
  Cookies,
  LocalStorage,
  SessionStorage,
}

@Injectable({
  providedIn: 'root'
})
export class CacheService {

  constructor() { }

  clear = (key: string,  storageType: StorageType = StorageType.LocalStorage) => {
    let storage: Storage;
    switch (storageType) {
      case StorageType.Cookies:
        throw "not implemented yet";
        break;
      case StorageType.LocalStorage:
        storage = window.localStorage;
        break;
      case StorageType.SessionStorage:
        storage ??= window.sessionStorage;
        break;
    }
    storage.removeItem(key);
  }

  Save = (key: string, value: any, storageType: StorageType = StorageType.LocalStorage) => {
    let storage: Storage;
    switch (storageType) {
      case StorageType.Cookies:
        throw "not implemented yet";
        break;
      case StorageType.LocalStorage:
        storage = window.localStorage;
        break;
      case StorageType.SessionStorage:
        storage ??= window.sessionStorage;
        break;
    }
    storage.setItem(key, JSON.stringify(value));
    return value;
  }

  Load = <T=any>(key: string, storageType: StorageType = StorageType.LocalStorage) => {
    let storage: Storage;
    switch (storageType) {
      case StorageType.Cookies:
        throw "not implemented yet";
        break;
      case StorageType.LocalStorage:
        storage = window.localStorage;
        break;
      case StorageType.SessionStorage:
        storage = window.sessionStorage;
        break;
    }
    const value = storage.getItem(key);
    return value ? JSON.parse(value) as T : null;
  }
}
