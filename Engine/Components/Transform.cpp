#include "Transform.h"
#include "Entity.h"

namespace Engine::transform {

	namespace {
		utls::vector<math::v3>		positions;
		utls::vector<math::v4>		rotations;
		utls::vector<math::v3>		scales;
	}// anonymous namespace

	component create(init_info info, game_entity::entity e) {
		assert(e.is_valid());
		const id::id_type entity_index{ id::index(e.get_id()) };

		if (positions.size() > entity_index) {
			rotations[entity_index] = math::v4(info.rotation);
			positions[entity_index] = math::v3(info.position);
			scales[entity_index] = math::v3(info.scale);
		}
		else {
			assert(positions.size() == entity_index);
			positions.emplace_back(info.position);
			rotations.emplace_back(info.rotation);
			scales.emplace_back(info.scale);
		}

		return component(transform_id{ (id::id_type)positions.size() - 1 });
		//return component{ transform_id{ game_entity::entity.get_id()}};

	}
	void remove(component c) {
		assert(c.is_valid());
	}

	math::v3 component::position() const {
		assert(is_valid());
		return positions[id::index(_id)];
	}
	math::v4 component::rotation() const {
		assert(is_valid());
		return rotations[id::index(_id)];
	}
	math::v3 component::scale() const {
		assert(is_valid());
		return scales[id::index(_id)];
	}
}