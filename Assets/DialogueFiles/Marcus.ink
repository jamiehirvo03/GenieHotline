=== marcusIntroduction ===
    Hello I'm Marcus calling to you on DAY ONE as a test! Press [SPACE] to continue to the next line.
    Test these questions?
    + [Yes]
        -> marcusQuestions
    + [No]
        ->DONE
        
=== marcusQuestions ===
    + [This is a test question]
        You just pressed Response One!
        -> marcusQuestions
        
    + [This is another test question]
        Response Two works!
        -> marcusQuestions
        
    + [A third question?]
        Response Three works too!
        -> marcusQuestions
        
    + [Last but not least]
        Response Four present and accounted for!
        -> marcusQuestions
        
=== hangUp ===
    \*you hang up*
    -> DONE