=== introduction ===
    Hey there, I've got a particularly unique problem and I wonder if you could be just what I need to solve it.
    
    * [Okay?]
    -> wishDescription
    
    * [Probably not...]
    Wow not even going to hear what it is?
    -> wishDescription
    
=== wishDescription ===
    Can you like... make me smarter?
    
    * [Yes]
    Awesome thanks!
    -> hangUp
            
    * [We don't do that]
    Ah dang. Well it was worth a shot I guess.
    -> hangUp
    
    * [Depends what you mean by that]
    Exactly what it sounds like, oh you need details? Sure. Ask away.
    -> questions
        
=== questions ===
    (Gather some more info or CLICK the phone again to hang up)
    
    + [Name, age, and occupation?]
    Name's Michelle, 37 years young! Oh I forgot to mention, I'm a mechanic!
    -> questions
    
    + [What's your reason for making this wish?]
    I just got this new client at work and they're super loaded, well atleast I'm assuming they're super loaded because they brought in this real classy european car.
    Anyways this could be a really big potential new customer but the problem is that it's an electric car, which really isn't my forte.
    -> questions
        
    + [Any other details that might help?]
    The make is an "Electron" if that helps.
    -> questions
        
=== hangUp ===
    /DISCONNECTED/
    -> DONE