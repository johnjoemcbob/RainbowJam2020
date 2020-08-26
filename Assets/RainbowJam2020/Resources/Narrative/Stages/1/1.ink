INCLUDE ../../variables.ink

This is character 1...
Take me to {character2}
that I might speak with them!

+ [1]
-> Port1
+ [2]
-> Port2
+ [3]
-> Port3
+ [7]
-> Port7
+ [8]
-> Port8

=== Port1 ===
This person cannot supply bread!
~ portrait1 = true

-> END

=== Port2 ===
Mama mia its the baker person!
Bread incoming!
~ win = true

-> END

=== Port3 ===
Not only do I have no bread,
I am a fascism and breads are illegal!
~ lose = true

-> END

=== Port7 ===
Cat woz ere

-> END

=== Port8 ===
I am number eight and I am the winner

-> END