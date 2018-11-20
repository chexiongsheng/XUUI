local _M = {}


function _M.collect(go)
    local infos = CS.XUUI.UGUIAdapter.Collector.Collect(go)
	local r = {}
	
	for i = 0, infos.Length - 1 do
	    local objs = infos[i]
        local t = {}
		for j = 0, objs.Length - 1 do
            table.insert(t, objs[j])
		end
        table.insert(r, t)
	end
    
    return r
end

return _M
