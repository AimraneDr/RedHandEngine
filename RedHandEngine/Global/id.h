#pragma once
#include "CommonHeaders.h"

namespace RedHandEngine::id {
using id_type = u32;
namespace details {
	constexpr u32		generation_bits{ 10 };//able to change
	constexpr u32		index_bit{ sizeof(id_type) * 8 - generation_bits };
	constexpr id_type	index_mask{ (id_type{1} << index_bit) - 1 };
	constexpr id_type	generation_mask{ (id_type{1} << generation_bits) - 1 };
}//internal namespace
constexpr id_type	invalid_id{ (id_type) -1 };
constexpr u32		min_deleted_elements { 1024 };

using generation_type = std::conditional_t<details::generation_bits <= 16, std::conditional_t<details::generation_bits <= 8, u8, u16>, u32>;
static_assert(sizeof(generation_type) * 8 >= details::generation_bits);
static_assert((sizeof(id_type) - sizeof(generation_type)) > 0);

constexpr bool
is_valid(id_type id) {
	return id != invalid_id;
}

constexpr id_type
index(id_type id) {
	id_type index{ id & details::index_mask };
	assert(index != details::index_mask);
	return index;
}

constexpr id_type
generation(id_type id) {
	return (id >> details::index_bit) & details::generation_mask;
}

constexpr id_type
new_generation(id_type id) {
	const id_type generation{ id::generation(id) + 1 };
	assert(generation < ((u64)1 << details::generation_bits) - 1);
	return index(id) | (generation << details::index_bit);
}

#if _DEBUG
namespace details {
	struct id_base {
		constexpr explicit id_base(id_type id) : _id{ id }{}
		constexpr operator id_type() const { return _id; }
	private :
		id_type _id;
	};
}
#define DEFINE_TYPED_ID(name)								\
		struct name final : id::details::id_base			\
		{													\
			constexpr explicit name(id::id_type id)			\
				:id_base{ id } { }							\
			constexpr name() : id_base{ 0 }{}				\
		};
#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif

};


