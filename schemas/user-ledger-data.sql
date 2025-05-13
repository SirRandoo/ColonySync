CREATE TABLE UserLedgerData
(
    user_id   UUID NOT NULL REFERENCES Users (user_id) ON DELETE CASCADE,
    ledger_id UUID NOT NULL REFERENCES Ledgers (ledger_id) ON DELETE CASCADE,
    balance   BIGINT,
    karma     SMALLINT,

    PRIMARY KEY (user_id, ledger_id)
);

CREATE INDEX Idx_User_Ledger_Data_User_Id ON UserLedgerData (user_id);
CREATE INDEX Idx_User_Ledger_Data_Ledger_Id ON UserLedgerData (ledger_id);

CREATE TYPE Morality AS ENUM ('super_bad', 'very_bad', 'bad', 'neutral', 'good', 'very_good', 'super_good');

CREATE TABLE UserTransaction
(
    user_transaction_id UUID PRIMARY KEY,
    name                TEXT      NOT NULL,
    user_ledger_user_id UUID      NOT NULL,
    user_ledger_id      UUID      NOT NULL,
    amount              BIGINT    NOT NULL,
    was_refunded        BOOLEAN DEFAULT FALSE,
    morality            Morality  NOT NULL,
    occurred_at         TIMESTAMP NOT NULL,
    product_id          TEXT NULL,
    product_type        ProductType,

    CONSTRAINT fk_user_ledger FOREIGN KEY (user_ledger_user_id, user_ledger_id) REFERENCES UserLedgerData (user_id, ledger_id) ON DELETE CASCADE,
    CONSTRAINT fk_product FOREIGN KEY (product_id, product_type) REFERENCES Products (product_id, product_type) ON DELETE CASCADE
);

CREATE INDEX Idx_User_Transaction_User_Ledger ON UserTransaction (user_ledger_user_id, user_ledger_id);
CREATE INDEX Idx_User_Transaction_Product ON UserTransaction (product_id, product_type);
