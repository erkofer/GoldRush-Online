--
-- Set a lock
--

local key     = KEYS[1] -- key
local ttl     = tonumber(ARGV[1]) -- ttl in ms
-- Returns {bool isLockAquired, int pttl}

local lockSet = redis.call('SETNX', key, '')

if lockSet == 1 then -- if we aquired the lock
    redis.call('PEXPIRE', key, ttl)
    return {true, ttl}
else -- lock was already taken
	local oldTtl = redis.call('PTTL', key)
	if oldTtl == -1 or oldTtl == -2 then
		oldTtl = 0
	end
    return {false, oldTtl}
end