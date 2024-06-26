#pragma comment(lib,"RedHandEngine.lib")

#define TEST_ENTITY_COMPONENT 1


#if TEST_ENTITY_COMPONENT
#include "TestEntityComponent.h"
#else
#error one of the tests need to be enabled
#endif


int main() {
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	engine_test test{};
	
	if (test.initialize()) {
		test.run();
	}

	test.shutdown();

}