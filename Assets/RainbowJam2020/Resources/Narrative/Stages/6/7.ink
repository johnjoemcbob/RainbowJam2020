INCLUDE ../../variables.ink

These are the words of Lukios, King of
Mycenae. Our neighbouring cities are beset
by encroaching raiders!
Thieves of the lowest order who envy our
wealth and splendor! Share your swords
with us! Let your blades be ours!
Let all know who is mighty and who is not!






+ [9]
-> Port9
+ [10]
-> Port10
+ [17]
-> Port17
+ [19]
-> Port19


=== Port9 ===
Seems I was too caught up in metaphor to
make my request specific. I accept your
shipment, if it was in earnest. But...
Do not toy with me. My temper is short,
my wrath terrible. I will execute these
foreign conspirators by my own power.

~ win = true

-> END

=== Port10 ===
Seems I was too caught up in metaphor to
make my request specific. I accept your
shipment, if it was in earnest. But...
Do not toy with me. My temper is short,
my wrath terrible. I will execute these
foreign conspirators by my own power.

~ win = true

-> END

=== Port17 ===
~ character7 = "Failure!"
~ portrait7 = false

Those soldiers will kill the rebels!
Try interpreting his request more
creatively...
He asked for "blades". Maybe he
should make his own!

~ lose = true

-> END

=== Port19 ===
~ character7 = "Failure!"
~ portrait7 = false

Someone else needs this!

~ lose = true

-> END

