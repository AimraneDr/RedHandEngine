#pragma once
#include "../Components/ComponentsCommon.h" 
#include "TransformComponent.h"
#include "ScriptComponent.h"

namespace RedHandEngine {
	namespace game_entity {
		DEFINE_TYPED_ID(entity_id);

		class entity {
		public:
			constexpr explicit entity(entity_id id) : _id{ id } {}
			constexpr entity() : _id{ id::invalid_id } {}
			constexpr entity_id get_id() const { return _id; }
			constexpr bool is_valid() const { return id::is_valid(_id); }
			transform::component transform() const;
			script::component script() const;
		private:
			entity_id _id;
		};
	}//game_entity namespace

	namespace script {
		class entity_script : public game_entity::entity {
		public:
			virtual ~entity_script() = default;
			virtual void Begin() {};
			virtual void Update(float) {};
		protected:
			constexpr explicit entity_script(game_entity::entity entity)
				:game_entity::entity{ entity } {}
		};

		namespace details {
			using script_ptr = std::unique_ptr<entity_script>;
			using script_creator = script_ptr(*)(game_entity::entity entity);
			using string_hash = std::hash<std::string>;
			u8 register_script(size_t, script_creator);

			template<class script_class> script_ptr create_script(game_entity::entity entity) {
				assert(entity.is_valid());
				return std::make_unique<script_class>(entity);
			}
		}
#define REGISTER_SCRIPT(TYPE)											\
	class TYPE;															\
	namespace {															\
		const u8 reg##TYPE												\
		{ RedHandEngine::script::details::register_script(						\
				RedHandEngine::script::details::string_hash()(#TYPE) ,			\
				&RedHandEngine::script::details::create_script<TYPE>) };		\
		}
	}//script namespace

}