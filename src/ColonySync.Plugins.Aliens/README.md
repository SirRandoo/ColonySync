# Humanoid Alien Races

This project contains the compatibility code for the mod "Humanoid Alien Races,"
a framework for creating additional kinds of pawns with their own racial features.

## Current Features

### Racial Restrictions

While this plugin is active, viewers whose pawn is a HAR pawn can’t purchase
traits, backstories, or any other restricted purchase types if their pawn can’t
naturally pawn with it. For instance, if a pawn cannot have the trait "sanguine,"
viewers will not be allowed to add said trait to their pawn. If a pawn is only
allowed to have a certain list of traits, like "kind," "brawler," "trigger-happy,"
etc., viewers will only be allowed to purchase traits within the said list.

## Notes

### Pawn Purchasing

ColonySync, the greater project, dynamically adds pawns who have an associated
`PawnKindDef`, which all pawns typically should have. So, while this plugin
isn’t required to purchase pawns added via the Humanoid Alien Races framework,
this plugin adds the required compatibility code that adds support for framework's
`PawnKindDef` subclass that provides racial restrictions. 
