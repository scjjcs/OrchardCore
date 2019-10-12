const faker = require('faker');
const fs = require('fs');

module.exports = {
    generateBlogData: (count = 10) => {
        let data = [];
        for (let i = 0; i < count; i++) {
            var name = faker.random.uuid();
            var text = faker.lorem.paragraphs();

            data.push({
                name,
                text
            });
        }

        fs.writeFileSync('./cypress/fixtures/blog-posts.json', JSON.stringify(data, null, 4));
    }
};
