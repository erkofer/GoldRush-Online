--
-- Set a lock
--

local key     = KEYS[1] -- key
local count   = tonumber(ARGS[1]) -- number to pop
local pop     = tonumber(ARGS[2])
if(pop == 0) then pop = 'LPOP' else pop = 'RPOP' end
-- Returns {bool isLockAquired, int pttl}

ret = {}
for i = 1, count do
	ret[i] = redis.call(side)
end

return ret;