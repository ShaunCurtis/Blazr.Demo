# Application Design

There is normally a gulf between the academic theoretical best design and the implemented design for a project.

That gap exists for two reasons:
1. Knowledge constraints.
2. Time constraints.

I can't bridge the time gap, but I can help bridge the knowledge gap.

There are some fundimental good practices you can apply that will save you time and help you build a better structured application.  The problem you face is that while there's plenty of theory [often regurgitated from one source to the next], the exmples are canned.  Real world examples are almost non existant.

The basis for this discussion is the Invoice solution.

First, an architecture.  I use *Clean Design*.  I split my projects into four domains:

1. UI Domain - the building blocks for thr UI/UX.
2. Presentation Domain - data management layer for the specific UI.
3. Core Domain - the entites and core business logic of the application
4. Infrastructure Domain - the interface into externally provided services such as data stores and Email services

UI => Presentation => Core <= Infrastructure.



Above these I have one or more *Deployment* projects.  

