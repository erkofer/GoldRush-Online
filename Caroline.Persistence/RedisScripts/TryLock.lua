--
-- Set a lock
--
local key     = KEYS[1] -- key
local ttl     = KEYS[2] -- ttl in ms
local content = KEYS[3] -- lock content

local lockSet = redis.call('SETNX', key, content)
local oldTtl;

if lockSet == 1 then -- if lock was set
    redis.call('PEXPIRE', key, ttl)
    return {lockSet, ttl}
else -- lock was already taken
    return {lockSet, redis.call('PTTL')}
end