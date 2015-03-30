--
-- Increments a value and sets an expiry on that key.
--
local key		= KEYS[1]	-- key
local increment	= ARGV[1]	-- increment value
local ttlMs		= ARGV[2]	-- time to live in milliseconds

local get = redis.call('INCRBY', key, increment);
redis.call('PEXPIRE', key, ttlMs)
return get;