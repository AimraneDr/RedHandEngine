#include "Script.h"
#include "Entity.h"

namespace Engine::script {
	namespace {
		utls::vector<details::script_ptr>		entity_scripts;
		utls::vector<id::id_type>				id_mapping;

		utls::vector<id::generation_type>		generations;
		utls::vector<script_id>					free_ids;
		using script_registry = std::unordered_map<size_t, details::script_creator>;
		script_registry& registry() {
			//NOTE: we put this static variable in a function because of the initializing order
			//		of static data, this way we can be certain that the data had been initialized 
			//		before accessing it
			static script_registry reg;
			return reg;
		}

		bool exists(script_id id) {
			assert(id::is_valid(id));
			const id::id_type index{ id::index(id) };
			assert(index < generations.size() && id_mapping[index] < entity_scripts.size());
			assert(generations[index] == id::generation(id));

			return (generations[index] == id::generation(id)) &&
				entity_scripts[id_mapping[index]] &&
				entity_scripts[id_mapping[index]]->is_valid();
		}
	}//anonymoss namespace
	namespace details {
		u8 register_script(size_t tag, script_creator func) {
			bool result{ registry().insert(script_registry::value_type{tag,func}).second };
			assert(result);
			return result;
		}
	}//details namespace



	component create(init_info info, game_entity::entity e) {
		assert(e.is_valid());
		assert(info.script_creator);

		script_id id;
		if (free_ids.size() > id::min_deleted_elements) {
			id = free_ids.front();
			//bool valid = game_entity::is_alive(entity{ id });
			assert(!exists(id));
			free_ids.pop_back();
			id = script_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else {
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
		}

		assert(id::is_valid(id));
		const id::id_type index{ (id::id_type)entity_scripts.size() - 1 };
		entity_scripts.emplace_back(info.script_creator(e));
		assert(entity_scripts.back() -> get_id() == e.get_id());
		id_mapping[id::index(id)] = index;

		return component{ id };
	}

	void remove(component c) {
		assert(c.is_valid() && exists(c.get_id()));
		const script_id id{ c.get_id() };
		const id::id_type index{ id_mapping[id::index(id)] };
		const script_id last_id{ entity_scripts.back()->script().get_id() };
		utls::erase_unordered(entity_scripts, index);
		id_mapping[id::index(last_id)] = index;
		id_mapping[id::index(id)] = id::invalid_id;
	}



}