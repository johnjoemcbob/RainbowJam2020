INCLUDE ../../variables.ink

We’ve rebalanced our economy thanks to the
agreements you helped set up with our
Hattusan friend!
Well, I can’t say the same for the rest of
the island, but one thing we all have an
excess of is seafood.
I’m in a generous mood, so perhaps we could
offer a relief shipment? Just point me
where you want it.




+ [2]
-> Port2
+ [3]
-> Port3


=== Port2 ===
Unorthodox, but they are in need.
I think I’m getting a read on you now.
Hardly an arm of the state, are you?



~ win = true

-> END

=== Port3 ===
~ character5 = "Failure!"
~ portrait5 = false
The shipment will be intercepted if it goes by official channels!

~ lose = true

-> END