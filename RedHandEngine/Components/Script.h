#pragma once
#include "ComponentsCommon.h"

namespace RedHandEngine::script {


	struct init_info
	{
		details::script_creator script_creator;
	};

	component create(init_info info, game_entity::entity e);
	void remove(component c);
}