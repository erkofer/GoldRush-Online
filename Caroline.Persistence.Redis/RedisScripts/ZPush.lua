--
-- Pushes a key onto a sorted set at the head or tail
--
local key		= KEYS[1]	-- key of a sorted set
local value		= ARGS[1]	-- The value to be inserted into the stack
local pushSide	= ARGS[2]	-- The side of the set. 0 = left, 1 = right
local scoreOffset=ARGS[3]	-- The value to be added to the score of the penultimate entry and used as the new values score
-- integer ret				-- 1 if the member was inserted, 0 if it already existed and its score was updated.

-- commands sourced from https://stackoverflow.com/questions/20017255

local command
local arg1
local arg2

if pushSide == 0 then
  command = 'ZREVRANGEBYSCORE'
  arg1 = '+inf'
  arg2 = '-inf'
elseif pushSide == 1 then
  command = 'ZRANGEBYSCORE'
  arg1 = '-inf'
  arg2 = '+inf'
else
  -- error
  return redis.error_reply('Argument 2 must equal 0 or 1 to denote pushing to the left or right of the sorted set.')
end

local adjacentScore = redis.call(command, key, arg1, arg2, 'WITHSCORES', 'LIMIT', 0, 1)[2]
return redis.call('ZADD', key, adjacentScore + scoreOffset)