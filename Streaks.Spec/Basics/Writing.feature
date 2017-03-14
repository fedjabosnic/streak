Feature: Writing


Scenario: Data that is uncommitted is not available to readers

   Given an empty streak
    When I write "100" bytes of data
     And I write "50" bytes of data
     And I write "25" bytes of data
     And I do not commit
    Then the reader should not see any data


Scenario: Data that is discarded is not available to readers

   Given an empty streak
    When I write "100" bytes of data
     And I write "50" bytes of data
     And I write "25" bytes of data
     And I discard
    Then the reader should not see any data


Scenario: Data that is committed is available to readers

   Given an empty streak
    When I write "100" bytes of data
     And I write "50" bytes of data
     And I write "25" bytes of data
     And I commit
    Then the reader should see "3" entries totalling "175" bytes


Scenario: Extended write scenario with multiple discards and commits

   Given an empty streak
    When I write "100" bytes of data
     And I commit
     And I write "50" bytes of data
     And I discard
     And I write "25" bytes of data
     And I commit
     And I write "10" bytes of data
     And I do not commit
    Then the reader should see "2" entries totalling "125" bytes