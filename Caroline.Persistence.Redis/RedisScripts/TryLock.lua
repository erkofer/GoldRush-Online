--
-- Set a lock
--

local key     = KEYS[0] -- key
local ttl     = ARGS[0] -- ttl in ms

-- Returns {bool isLockAquired, int pttl}

local lockSet = redis.call('SETNX', key, "")
local oldTtl;

if lockSet == 1 then -- if we aquired the lock
    redis.call('PEXPIRE', key, ttl)
    return {lockSet, ttl}
else -- lock was already taken
	oldTtl = redis.call('PTTL');
	if(oldTtl == -1 || oldTtl == -2)
		oldTtl = 0;
    return {lockSet, oldTtl}
end