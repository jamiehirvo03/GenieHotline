=== introduction ===
    Hey, the names Marcus, it true you grant wishes?
    
    * [Yes]
    -> wishDescription
    
    * [No]
    Oh great, what a waste of time.
    -> hangUp
    
=== wishDescription ===
    Great, I pushed myself a little bit too hard last week at work and my back's been killing me ever since. 
    You able to do something about that?
    
    * [Consider it done]
    -> hangUp
            
    * [Sorry, I can't help you]
    Wow some kind of Genie you are.
    Thanks for nothing.
    -> hangUp
    
    * [Yeah, but I'll need some more details]
    Okay sure, but make it quick. I wasn't kidding about the pain.
    -> questions
        
=== questions ===
    (Gather some more info or CLICK the phone again to hang up)
    
    + [Name, age, and occupation?]
    Marcus, 34, Bricklayer.
    -> questions
    
    + [What's your reason for making this wish?]
    What do you think jackass?
    Sorry that was a bit much, but seriously like I said I'm obviously in a load of pain here.
    I've tried all the painkillers I can get my hands on but it still isn't good enough.
    -> questions
        
    + [Any other details that might help?]
    No it's pretty straight forward dude...
    -> questions
        
=== hangUp ===
    /DISCONNECTED/
    -> DONE