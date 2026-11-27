=== introduction ===
    This is the so called Genie Hotline, right?
    
    * [Yes]
    -> wishDescription
    
    * [No]
    Stop lying, I know for a fact I dialed the right number.
    -> wishDescription
    
=== wishDescription ===
    I really don't trust this new over the phone setup you got going, I have some really imporant business that needs sorting out and I'd rather handle this face to face.
    Give me one of those lamps you guys always use, the kind that like everyone that's ever existed knows about.
    
    * [I think I can help you with that]
    You think? well whatever I don't have time for this, I better get my lamp.
    -> hangUp
            
    * [Sorry, I can't help you]
    I knew this was a scam, what kind of Genie doesn't deal with lamps?
    -> hangUp
    
    * [Yeah, but I'll need some more details]
    What else could you possibly need to know?
    -> questions
        
=== questions ===
    (Gather some more info or CLICK the phone again to hang up)
    
    + [Name, age, and occupation?]
    Fine. It's Jerry and I'm 46. As for occupation that's none of your business.
    -> questions
    
    + [What's your reason for making this wish?]
    Well mainly so I don't have to deal with nonsense like this, how much do you need to know for such a simple request?
    -> questions
        
    + [Any other details that might help?]
    No, that's literally it.
    -> questions
        
=== hangUp ===
    /DISCONNECTED/
    -> DONE