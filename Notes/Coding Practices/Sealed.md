# Sealed By Default

In many languages objects are sealed by default.  You can't inherit from them, unless you explicitly unseal them.  

C# made the opposite decision.

In the latest Net versions, the Dev team have been busy modifying everything they can to sealed.

Their reason is compiler improvements mean sealed classes have gained a small performance edge over unsealed classes.

You should also consider this: Is you class designed for inheritance?  Only open it if it is.


So, sealing by default will provide a small performance benefit and prevent inadvertant inheritance.
