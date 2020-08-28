INCLUDE ../../variables.ink

This is the high priestess of Knossos,
requesting bronze. We’ve never needed
an army, but piracy is on the rise.
...also, a dangerous outlaw from our
island has been at large lately. She’s
always trying to disturb the peace...
If you know anything, I’d accept pointing me to her in place of bronze. But nothing short of one of those.


+ [19]
-> Port19
+ [2]
-> Port2
+ [9]
-> Port9



=== Port19 ===
Idamate bless you. We will be safe with this...are you certain you have heard nothing of the renegade?


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
