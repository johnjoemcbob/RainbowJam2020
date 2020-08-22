VAR portrait = false
VAR win = false
VAR lose = false

This is character 1...
so up here is the introduction, the person
calls and asks the operator for a bread

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
dev note: all portraits enabled now
~ portrait = true

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
I am number seven :)

-> END

=== Port8 ===
I am number eight and I am the winner

-> END