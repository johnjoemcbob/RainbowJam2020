INCLUDE ../../variables.ink

I hope this message finds you well. We’ve managed to rebalance our economy thanks to the agreements you helped set up with our Hattusan friend. Well, I can’t say the same for the rest of the island, but one thing we all have an excess of is seafood.

I’m in a generous mood, and I hear you’re starting to experience troubles down in Egypt too, so perhaps we could offer a relief shipment? Just point me where you want it.




+ [2]
-> Port2
+ [3]
-> Port3


=== Port2 ===
An unorthodox response, but they do seem in need. It will be sent at once. You know, I think I’m starting to get a read on you. You’re not an arm of the state at all anymore, are you?



~ win = true

-> END

=== Port3 ===
~ character5 = "Failure!"
~ portrait5 = false
The shipment will be intercepted if it goes by official channels!

~ lose = true

-> END