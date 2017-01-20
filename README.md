# super-spaceballs
Game project at HIS



En match består av ett antal rundor. I varje runda så får spelaren göra ett drag per robot.
Ett drag består av flera handlingar/kommandon.
# Todo
  * SoundManager GameObject som är man kommer åt med en singletoninstance. SoundManager
  har en funktion PlaySound(Sound s) där Sound är ett enum med flera värden.
  * Robot GameObject som inte borde uppdateras i planeringsfasen. Robot skulle kunna
  ärva från en klass så att det i framtiden är möjligt att lägga till Robotar med
  andra slags handlingar.
  * Handlingar ärver från en abstrakt överklass så att vi kan spara ner alla handlingar
  i en och samma lista.
  * Rundor håller logiken för att låta spelare välja och spara ner handlingar.
  Rundor sparar ner en lista på alla robotars handlingar.
  * GameControll GameObject som håller alla rundor och skapar en ny runda när spelfasen
  har "spelats klart".





# Kodriktlinjer
* Måsvingar på samma rad som funktion
* Dela upp kod i sektioner: privata variabler > publika variabler > properties > privata funktioner > publika funktioner > protected funktioner
* Ha en avskiljare med rubrik över varje sektion
