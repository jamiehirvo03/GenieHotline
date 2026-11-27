=== introduction ===
    Opening statement/question.
    
    * [Yes]
    -> wishDescription
    
    * [No]
    Response before hanging up.
    -> wishDescription
    
=== wishDescription ===
    Wish description/preamble here. End with questioning if it can be done.
    
    * [Positive answer]
    Answer here.
    -> hangUp
            
    * [Negative answer]
    Answer here.
    -> hangUp
    
    * [Positive, but enter questioning phase]
    Answer here.
    -> questions
        
=== questions ===
    (Gather some more info or CLICK the phone again to hang up)
    
    + [Name, age, and occupation?]
    Answer here.
    -> questions
    
    + [What's your reason for making this wish?]
    Answer here.
    -> questions
        
    + [Any other details that might help?]
    Answer here.
    -> questions
        
=== hangUp ===
    /DISCONNECTED/
    -> DONE