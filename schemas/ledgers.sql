/*
    The table schema for ledgers.
 */
CREATE TABLE Ledgers
(
    /*
        The unique id of the ledger.
     */
    ledger_id     UUID PRIMARY KEY,

    /*
        The human-readable name of the ledger.
     */
    name          TEXT      NOT NULL,

    /*
        The moment in time the ledger was last modified.
     */
    last_modified TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CHECK ( last_modified <= CURRENT_TIMESTAMP )
);
