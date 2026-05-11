INSERT OR IGNORE INTO Flags (Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt) VALUES
    ('new-checkout-flow',    'New checkout flow',          1, 100, datetime('now'), datetime('now')),
    ('dark-mode',            'Dark mode',                  1,  50, datetime('now'), datetime('now')),
    ('ai-suggestions',       'AI suggestions in editor',   1,  25, datetime('now'), datetime('now')),
    ('experimental-pricing', 'Experimental pricing page',  0,   0, datetime('now'), datetime('now')),
    ('legacy-export',        'Legacy CSV export',          1, 100, datetime('now'), datetime('now'));
