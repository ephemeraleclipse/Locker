using System;
using System.Collections.Generic;
using System.Threading;

namespace Locker
{
    public class Locker
    {

        #region Locks

        private static Object _masterLock = new object();

        private static Dictionary<String, Object> _locks;

        private static Dictionary<String, Object> Locks
        {
            get
            {
                if (_locks == null)
                    _locks = new Dictionary<String, Object>();
                return _locks;
            }
        }

        #endregion

        #region Keys

        private static List<String> _keys;

        private static List<String> Keys
        {
            get
            {
                if (_keys == null)
                    _keys = new List<String>();
                return _keys;
            }
        }

        #endregion

        #region Methods

        public static void AddLock(String name)
        {
            lock (_masterLock)
            {
                if (Locks.ContainsKey(name))
                {
                    throw new Exception(String.Format("Specified lock with name {0} already exists", name));
                }
                else
                {
                    for (int i = 0; i < Keys.Count; i++)
                    {
                        if (name.CompareTo(Keys[i]) < 0)
                        {
                            lock (Locks[Keys[i]])
                            {
                                Keys.Insert(i, name);
                            }
                            break;
                        }
                    }
                    if (!Keys.Contains(name))
                    {
                        Keys.Add(name);
                    }
                    Locks.Add(name, new object());
                }
            }
        }

        public static void RemoveLock(String name)
        {
            lock (_masterLock)
            {
                if (Locks.ContainsKey(name))
                {
                    lock (Locks[name])
                    {
                        Keys.Remove(name);
                        Locks.Remove(name);
                    }
                }
                else
                {
                    throw new Exception(String.Format("Specified lock with name {0} does not exist", name));
                }
            }
        }

        private static Boolean Lock(String name)
        {
            Boolean locked = false;
            int index = 0;

            lock (_masterLock)
            {

                if (Locks.ContainsKey(name))
                {
                    index = Keys.IndexOf(name);

                    for (int i = 0; i < index; i++)
                    {
                        Monitor.Enter(Locks[Keys[i]]);
                    }
                    Monitor.Enter(Locks[name]);
                    locked = true;
                }
                else
                {
                    locked = false;
                    throw new Exception(String.Format("Specified lock with name {0} does not exist", name));
                }
            }

            return locked;
        }

        private static void Unlock(String name)
        {
            int index = 0;

            lock (_masterLock)
            {
                if (Locks.ContainsKey(name))
                {
                    index = Keys.IndexOf(name);

                    for (int i = 0; i < index; i++)
                    {
                        Monitor.Exit(Locks[Keys[i]]);
                    }
                    Monitor.Exit(Locks[name]);
                }
                else
                {
                    throw new Exception(String.Format("Specified lock with name {0} does not exist", name));
                }
            }
        }

        public static void Lock(String name, Action work)
        {
            try
            {
                if (Lock(name))
                    work();
            }
            finally
            {
                Unlock(name);
            }

        }

        public static void Reset()
        {
            lock (_masterLock)
            {
                Locks.Clear();
                Keys.Clear();
            }
        }

        #endregion

    }
}
