--
-- Gets the existing value, sets it to the new value and sets an expiry on that key.
--
local key		= KEYS[1]	-- key
local rate		= ARGS[1]	-- The number of requets allowed
local duration	= ARGS[2]	-- The duration in seconds
-- integer ret				-- 1 if the key has not exceeded its rate limit, otherwise 0

// get time in seconds;
local time = redis.call('TIME')[1]

// currentRate is an expirable key with the value format
// "timeRateTrackingStarted:numTimesCalled"
local currentRate = redis.call('GET', key)


// key could not exist or be expired
// in that case set up the key with expire
if currentRate == nil then
	redis.call('SETEX', key, duration, time + ":1")
	return 1
end

// key exists, parse it
local ratingStarted, local timesCalled = string.gmatch(currentRate, '([^,]+)')
ratingStarted = tonumber(rateStarted);
timesCalled = tonumber(timesCalled);

if time > ratingStarted + duration then
	// rating restarted
	redis.call('SETEX', key, duration, time + ":1")
	return 1
end

// too early to restart rating, check to see if we can increment the existing rating
if timesCalled => rate then
	// we may increment the existing rating
	local newTimesCalled = timesCalled + 1
	redis.call('SETEX', key, duration, ratingStarted + ":" + newTimesCalled)
	return 1;
end

// rate limited
return 0;