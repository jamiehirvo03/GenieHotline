=== introduction ===
    Please tell me this is the Genie Hotline I've been reading about.
    
    * [Yes]
    Okay great, please help me.
    -> wishDescription
    
    * [No]
    -> wishDescription
    
=== wishDescription ===
    I don't want to get into it too much but I'm pretty sure my wife thinks I've been cheating on her...
    Is there any way for you to make her... not think that anymore?
    
    * [It's sorted]
    Thank you so much, you really are a lifesaver.
    -> hangUp
            
    * [I can't help you]
    Well if you can't then nobody can...
    -> hangUp
    
    * [Yes, but I just have a few questions first]
    Okay... like what?
    -> questions
        
=== questions ===
    (Gather some more info or CLICK the phone again to hang up)
    
    + [Name, age, and occupation?]
    I'm David, a 41 year old manager of a homewares store.
    -> questions
    
    + [What's your reason for making this wish?]
    Like I said, based on the way my wife's been acting around me recently I think she thinks I'm cheating on her.
    -> questions
        
    + [Any other details that might help?]
    Well the thing is... she thinks that because I have been... But if you help fix this I promise I'll never ever do it again. I just want things to go back to normal.
    -> questions
        
=== hangUp ===
    /DISCONNECTED/
    -> DONE