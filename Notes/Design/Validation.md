# The Validation Conundrum

Validation is a topic that generates a lot of discussion [and disagreement].

1. Where do you do it?
2. How do you do it?
3. When do you do it?

The most important question is often not asked. Why?  Understand why and you will answer the when, how and where. 

There are three principle areas to consider:

1. Infrastructure Validation - checking data received from API's and back end services
is valid before it gets transferred to the domain entities.
2. Presentation/UI Validation - checking User entered data is valid before using it in domain entities.
3. Domain Processing Validation - validating data entities from Domain logic processes.

Lets deal with these.

## Immutability

Immutable objects are a cornerstone for controlling validation.  I

## Intrastructure Validation

Mapping to objects is basically about verifying data type mappings.

The two key considerations are:

1. Are the data types correct between the database query and the mapped object? Mapping a `long` onto an `int` may cause problems at some point.
2. Are nulls dealt with correctly?  Make sure that mapped properties are nullable if they need to be.



