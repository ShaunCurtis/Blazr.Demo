# Design Philosophy

The application is build using a fusion of different frameworks and patterns.

The overriding structure is based on Clean Design.

Each layer is represented by a project, with tight inheritance rules to apply the Clean Design principles.

This framework mixes Functional Design with OOP.

You will see immutable value types used a lot.  My data objects are `readonly record struct` by default.  Business logic applies transforms to immutable data objects.
