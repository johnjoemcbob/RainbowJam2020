INCLUDE ../../variables.ink

This is the high priestess of Knossos speaking. Requesting a shipment of bronze--we’ve never much needed an army, but our waters are getting dangerous with all the increased piracy.

...also, there’s a dangerous outlaw from our island I’ve heard has been at large lately. She’s always trying to disturb the peace...if you know anything, I’d accept pointing me to her in place of the bronze. But nothing short of one of those.


+ [19]
-> Port19
+ [2]
-> Port2
+ [9]
-> Port9



=== Port19 ===
Idamate bless you. We will be safe from the foreign marauders with this...are you certain you have heard nothing of the renegade?


~ win = true

-> END


=== Port2 ===
~ character5 = "Failure!"
~ portrait5 = false
You gave the refugees away!

~ lose = true

-> END

=== Port9 ===
~ character5 = "Failure!"
~ portrait5 = false
Asujua said she won't accept anything less than bronze! Now who might be more flexible...

~ lose = true

-> END
