local setmetatable, getmetatable, rawset, rawget, pairs, type, ipairs = setmetatable, getmetatable, rawset, rawget, pairs, type, ipairs
local insert = table.insert
local find, format, sub, gsub = string.find, string.format, string.sub, string.gsub

local function parse_path(path)
    if not path or path == '' then error('invalid path:' .. tostring(path)) end
    --print('start to parse ' .. path)
    local result = {}
    local i, n = 1, #path
    while i <= n do
        local s, e, split1, key, split2 = find(path, "([%.%[])([^%.^%[^%]]+)(%]?)", i)
        if not s or s > i then
            --print('"'.. sub(path, i, s and s - 1).. '"')
            insert(result, sub(path, i, s and s - 1))
        end
        if not s then break end
        if split1 == '[' then
            if split2 ~= ']' then error('invalid path:' .. path) end
            key = tonumber(key)
            if not key then error('invalid path:' .. path) end
            --print(key)
            insert(result, key)
        else
            --print('"'.. key .. '"')
            insert(result, key)
        end
        i = e + 1
    end
    --print('finish parse ' .. path)
    return result
end

local function get_by_keys(data, keys)
    for _, key in ipairs(keys) do
        if type(data) ~= 'table' then return end
        data = data[key]
    end
    return data
end

local function parse_expression(result, chunk)
  local i, n = 1, #chunk
  while i <= n do
    local s, e, expr = find(chunk, "$(%b{})", i)
    if not s or s > i then
      insert(result, format("_put(%q)", sub(chunk, i, s and s - 1)))
    end
    if not s then break end
    insert(result, format("_put(%s)", sub(expr, 2, -2)))
    i = e + 1
  end
end

local load = loadstring or load
local concat, insert, tostring = table.concat, table.insert, tostring

local do_render
if setfenv and not math.type then -- lua5.1
    function do_render(render, env, _put)
        setfenv(render, env)
        local status, err = pcall(render, _put)
        if not status then return error(err) end
    end
else
    function do_render(render, env, _put)
        local status, err = pcall(render, _put, env)
        if not status then return error(err) end
    end
end

local function compile_template(s)
    local result = {"local _put, _ENV = ..."}
    parse_expression(result, s)
    local render, err = load(concat(result), "string_template")
    if not render then return error(err) end
    return function(env)
        local t = {}
        do_render(render, env, function(s)
            if s ~= nil then insert(t, tostring(s)) end
        end)
        return concat(t)
    end
end

local function is_template(s)
    return find(s, "$%b{}")
end

return {
    parse_path = parse_path,
    get_by_keys = get_by_keys,
    compile_template = compile_template,
    is_template = is_template,
}