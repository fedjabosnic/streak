Feature: Reading


Scenario: Reading an entry that does exist

   Given a streak with "6" entries
    When I try to read entry "3"
    Then there should be no error

# TODO: Currently this blocks until the data is available - what do we want?

#Scenario: Reading an entry that doesn't exist
#
#   Given a streak with "6" entries
#    When I try to read entry "9"
#    Then there should be an error
#

Scenario: Reading sequentially

   Given a streak with "4" entries
    When I read the entries sequentially
    Then I should read the correct data


Scenario: Reading randomly

   Given a streak with "4" entries
    When I read the entries randomly
    Then I should read the correct data


Scenario: Reading in reverse

   Given a streak with "4" entries
    When I read the entries in reverse
    Then I should read the correct data

