Feature: Semantics


Scenario: Multiple readers are supported

   Given a reader is already open
    When I open another reader
    Then there should be no error


Scenario: Multiple writers are not supported

   Given a writer is already open
    When I open another writer
    Then there should be an error
     And it should say "Unable to take write lock on the streak"

