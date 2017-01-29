### super-spaceballs
Game project at HIS



En match består av ett antal rundor. I varje runda så får spelaren göra ett drag per robot.
Ett drag består av flera handlingar/kommandon.
## Todo
- En TurnHandler som alltid funkar lokalt med två spelare, i replay och i nätverk.


#
* Menu som aktiveras/avaktiveras av GameBehaviour.done
* Man ska kunna gå från ett aktiverat PlayBehaviour(alltså att man spelar) till menyn och sedan tillbaka. done
* PlayBehaviour ska aktiveras, turnhandlers instanseras av PB och spelet hålls igång av PB. done




## Kodriktlinjer

# Allmänna riktlinjer
* Dela upp kod i sektioner: privata variabler > publika variabler > properties > privata funktioner > publika funktioner > protected funktioner.
* Ha en avskiljare med rubrik över varje sektion.
* Funktioner och properties har upper camel case, variabler har lower camel case.
* Försök undvika att använda konstanter (magiska tal, text etc) direkt i koden. Använd variabler så mycket som möjligt.

# Riktlinjer för Unity
* Ge aldrig ett fält i inspectorn ett gameobject som är en aktiv instans p.g.a. risken att den kan tas bort. **Ge alltså bara prefabs av gameobjects till fält i inspectorn.** Alla prefabs har scene-namnet "Null" (vilket fås genom t.ex. someGameObject.scene.name). **Döp inte en scene till "Null"**. Genom att i funktionen OnValidate() kolla om scene-nmanet är null kan man avgöra om gameobjectet är en prefab eller inte. Om Det inte är det ska det göras tydligt att den som använder fältet har gjort fel, t.ex. genom ett meddelande i Debuggern. 
* Utveckla i egna Unity-scener.
* Skriv kod som förebygger null-referenser. Om det i ett visst kodstycke finns risk för en null-referens så ska åtgärder göras för att förhindra att den används. Detta kan göras genom t.ex. if-satser.
* Ändra inte namn på tags utan att samtala med gruppen.
