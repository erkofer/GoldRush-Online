--
-- Gets the existing value, sets it to the new value and sets an expiry on that key.
--
local key		= KEYS[1]	-- key
local newVal	= ARGS[1]	-- new Set value
local ttlMs		= ARGS[2]	-- time to live in milliseconds

local get = redis.call('GETSET', key, newVal);
-- GETSET resets the key expire, but expire could have different values on each call
redis.call('PEXPIRE', key, ttlMs)
return get;