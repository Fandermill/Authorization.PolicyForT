

TODO:
----
[ ] Record every evaluated requirement handler into one result
[ ] Make a base AuthorizationContextFactory(?)
[ ] Cache reflection operations in RequirementHandlerProvider




====================
Requirements deel
====================


IAuthorizationRequirement
-------------------------
Tagging interface voor de objectjes die de requirements definieren


IAuthorizationHandler<T, TRequirement>
--------------------------------------
Handler object die de requirement checkt tegen de gegeven context




IPolicy<T>
----------
Heeft 1 requirement of een collectie van requirements die samen
de hele policy vormen.
AbstractPolicy<T> is een base class met een IRequirementBuilder prop.

IRequirementBuilder / BuilderExtensions
---------------------------------------
Extensie methods om in fluent code requirements op te stellen.
Inclusief AllOf, AnyOf, AndAllOf, AndAnyOf, OrAllOf, OrAnyOf
en voor een Func<> (= DelegaRequirement)



IAuthorizationHandlerExecutor<T>
--------------------------------
Gaat alle IAuthorizationHandlers<T, TRequirement> van een
IAuthorizationRequirement bij lang en geeft success terug
bij eerste de beste handler die success geeft.

AuthorizationHandlerExecutor<T>
-------------------------------
Deze implementatie maakt via Reflectie een Type voor de handler
van de requirement en vraagt die op via de IServiceProvider.








====================
Root deel
====================


IAuthorizationContextFactory<T>
-------------------------------
Maakt een AuthorizationContext op basis van een gegeven T.
Er moet een basis factory komen die simpelweg de T instelt.
Maar wat doen we met de IPrincipal? Iets om te implementeren
door de client.

AuthorizationContext<T>
-----------------------
Object die data samenbrengt tot in 1 context.
Dus een T en een IPrincipal

AuthorizationResult
-------------------
todo.

IPrincipal
----------
Interface om het gebruikerobject te taggen


IRequestAuthorizer<T>
---------------------
Gaat de requirements in de policies na voor T met de
gemaakte context.






IRequirement                    IAuthorizationRequirement
IRequirementHandler             IAuthorizationHandler<T, TRequirement>
IPolicy                         IPolicy<T>
IRequirementBuilder             IRequirementBuilder / BuilderExtensions
IHandlerInvoker                 IAuthorizationHandlerExecutor<T>
IHandlerTypeProvider             ... new ... builds Type to request from IServiceProvider
                                AuthorizationHandlerExecutor<T>
IAuthorizationContextFactory<T> IAuthorizationContextFactory<T>
AuthorizationContext<T>         AuthorizationContext<T>
AuthorizationResult             AuthorizationResult
IPrincipal                      IPrincipal
IAuthorizer<T>                  IRequestAuthorizer<T>