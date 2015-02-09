--
-- Increments a value and sets an expiry on that key.
--
local key		= KEYS[0]	-- key
local increment	= ARGS[0]	-- increment value
local ttlMs		= ARGS[1]	-- time to live in milliseconds

local get = redis.call('INCRBY', key, increment);
redis.call('PEXPIRE', key, ttlMs)
return get;