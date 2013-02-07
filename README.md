Locker
======

Locker is a class with only static methods, the methods that are exposed are AddLock, RemoveLock, Reset and Lock. The purpose of this class is to replace c# syntax

Object obj = new Object();  
lock(obj)  
{  
    codes  
}  

and to safely share the locks with all classes without the fear of deadlock. The Lock method provides similar functionality. Usage:

Locker.AddLock("Lock name");  
Locker.Lock("Lock name", () =>  
{  
    codes  
});  

Lock method guarantees the lock specified by name and all lock lower than it in an alphabetical order are locked and unlocked when entering and leaving the code block. The lock names should be previously defined in a resource file. Any lock dependency should be taken into consideration when naming the locks, for example: if a lock requires another lock to be locked for it to function, the latter should be named something like "a" and the first lock should be named something like "b". Reason being the Lock method will lock in alphabetical order.
The Reset method in Locker will clear the Locks and Keys so there isn't any old locks left in Locker. 
Note: if a lock with a name that's not found in the Locker is to be Locked or Unlocked, exceptions will be thrown. Similarly, if a lock with a name that already exists in the Locker is to be added to the locker, exceptions will be thrown.
