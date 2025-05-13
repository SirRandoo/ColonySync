# Android Tiers

This project contains the compatibility code for the mod "Android Tiers," a mod
that adds a new pawn kind called "Androids."

## Current Features

### Surgery Integration

Since Android Tiers converts the typical surgery interface for its respective pawns
into a "crafting" recipe instead of a "surgery," this plugin is necessary for viewers
to be capable of purchasing and queuing a surgery for the pawn. Additionally, this
plugin is responsible for ensuring pawns who aren’t androids can’t purchase and queue
android surgeries.

### Heal Integration

Since Android Tiers contains some `Hediff`s that are critical to features of the mod,
this plugin provides special handling of these hediffs. Normally, ColonySync’s healing
feature would heal them since their definitions don’t contain the correct metadata to
ensure they aren’t healed by ColonySync, and from the game’s "healer mech serum" item,
which ColonySync’s healing code is based off of.

## Notes

### Pawn Purchasing

Androids can be purchased naturally by the greater ColonySync mod, as the Androids
pawn definitions are dynamically discovered at runtime.
